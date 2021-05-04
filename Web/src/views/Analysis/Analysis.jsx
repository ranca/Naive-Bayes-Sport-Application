import React, { Component } from 'react';
import { connect } from 'react-redux';
import { GetNumberWithOrdinal, IsNullOrUndefined } from 'common';
import { Dropdown, Header, Image, Menu, Popup, Segment, Statistic, Tab, Table } from 'semantic-ui-react';
import { LabelSeries, MarkSeries, VerticalBarSeries, FlexibleXYPlot, VerticalGridLines, HorizontalGridLines, RadialChart, XAxis, YAxis } from 'react-vis';
import ColorScheme from 'color-scheme';

import { getLogo } from '../../util/assets';

import './Analysis.scss';

import { Lineup, Loader } from '../../components';
import * as actions from '../../services/analysis';

class Analysis extends Component {

  constructor(props) {
    super(props);
    this.state = {
      'category': 'points'
    };
    this.onCategoryChange = this.onCategoryChange.bind(this);
  }

  componentDidMount() {
    this.props.getAnalysis();
  }

  onCategoryChange(event, data) {
    this.setState({
      category: data.value
    });
  }

  createStatistic(text, value) {
    return (
      <Statistic>
        <Statistic.Value>{value}</Statistic.Value>
        <Statistic.Label>{text}</Statistic.Label>
      </Statistic>);
  }

  render() {
    const { analysis } = this.props;
    const { category } = this.state;
    if (IsNullOrUndefined(analysis)) {
      return (<Loader />);
    }
    var scheme = new ColorScheme();
    scheme.from_hue(20).scheme('mono').variation('soft');
    const panes = analysis.map((team, index) => {
      var maxValue = 0;
      var teamPie = [];
      var teamPieConverted = team.rosterStats.filter((p) => { return p.stats.points > 0; }).sort((a, b) => { return a.stats[category] - b.stats[category]; }).map((ps) => {
        if (ps.stats[category] > maxValue) {
          maxValue = ps.stats[category];
        }
        return {
          'label': ps.fullName,
          'subLabel': ps.stats[category],
          'angle': ps.stats[category]
        };
      });
      teamPieConverted.reduce(function(res, value) {
        var id = value.angle < maxValue / parseFloat(5) ? '' : value.label;
        if (!res[id]) {
          res[id] = { label: id, subLabel: 0, angle: 0 };
          teamPie.push(res[id]);
        }
        res[id].angle += value.angle;
        res[id].subLabel += value.subLabel;
        return res;
      }, {});
      var goalieGraph = team.rosterStats.filter((p) => { return p.stats.points == null; }).sort((a, b) => { return a.stats.wins - b.stats.wins; }).map((ps) => {
        return {
          'label': ps.fullName,
          'subLabel': ps.stats.wins,
          'angle': ps.stats.wins,
        };
      });
      var teamGraph = team.rosterStats.filter((p) => { return p.stats.points !== null; }).map((ps) => {
        return {
          'label': ps.fullName,
          'x': ps.stats.games,
          'y': ps.stats[category],
          'rotation': 10
        };
      });
      return {
        menuItem: (
          <Menu.Item key={team.id}>
            {index + 1}. <Image avatar src={getLogo(team.id)} /> {team.team.name}
          </Menu.Item>
        ), render: () =>
          <Tab.Pane>
            <Header as='h1' className="team-header"><img className="mid-logo" src={getLogo(team.id)} alt={`img${team.id}${team.team.name}`} />{team.team.name}</Header>
            <Statistic.Group widths='5'>
              {this.createStatistic('League', GetNumberWithOrdinal(team.leagueRank))}
              {this.createStatistic('League Home', GetNumberWithOrdinal(team.leagueHomeRank))}
              {this.createStatistic('League Road', GetNumberWithOrdinal(team.leagueRoadRank))}
              {this.createStatistic('League Last 10', GetNumberWithOrdinal(team.leagueL10Rank))}
              {this.createStatistic('League Powerplay', GetNumberWithOrdinal(team.ppLeagueRank))}
            </Statistic.Group>
            <Statistic.Group widths='5'>
              {this.createStatistic('Division', GetNumberWithOrdinal(team.divisionRank))}
              {this.createStatistic('Division Home', GetNumberWithOrdinal(team.divisionHomeRank))}
              {this.createStatistic('Division Road', GetNumberWithOrdinal(team.divisionRoadRank))}
              {this.createStatistic('Division Last 10', GetNumberWithOrdinal(team.divisionL10Rank))}
              {this.createStatistic('Division Powerplay', GetNumberWithOrdinal(team.ppDivisionRank))}
            </Statistic.Group>
            <Table>
              <Table.Header>
                <Table.Row>
                  {team.stats.map((stat) => {
                    return (<Popup content={stat.description} key={stat.title} trigger={<Table.HeaderCell key={stat.title + team.id}>{stat.title}</Table.HeaderCell>}></Popup>);
                  })}
                </Table.Row>
              </Table.Header>
              <Table.Body>
                <Table.Row>
                  {team.stats.map((stat) => {
                    return (<Table.Cell key={stat.title + team.id}>{stat.value}</Table.Cell>);
                  })}
                </Table.Row>
              </Table.Body>
            </Table>
            <div>
            <FlexibleXYPlot height={300} xType="ordinal" yDomain={[0, 100]}>
              <VerticalGridLines />
              <HorizontalGridLines />
              <XAxis />
              <YAxis />
              <VerticalBarSeries data={team.rankingsGraph}></VerticalBarSeries>
            </FlexibleXYPlot>
            </div>
            <div className='graph-container'>
              <div className='filter-container'>
              <h4>Distribution among players:</h4>
              <Dropdown
                placeholder='Category'
                selection
                onChange={this.onCategoryChange}
                options={[{key: 'points', text: 'Points', value: 'points'}, {key: 'goals', text: 'Goals', value: 'goals'}, {key: 'assists', text: 'Assists', value: 'assists'}]}
              />
              </div>
              <RadialChart
                animation
                className='radial-graph'
                labelsRadiusMultiplier={0.99}
                showLabels
                data={teamPie}
                colorType={'category'}
                colorRange={scheme.colors().map((color)=>{
                  return `#${color}`;
                })}
                width={500}
                radius={200}
                height={500} />
              <FlexibleXYPlot  yDomain={[0, maxValue + 5]} xDomain={[0, 60]} height={500}>
                <XAxis title='Games' />
                <YAxis title='Points' />
                <MarkSeries
                  data={teamGraph}
                  opacity={0.5}
                  size={5}
                />
                <LabelSeries labelAnchorX='end' labelAnchorY='middle' animation allowOffsetToBeReversed data={teamGraph} />
              </FlexibleXYPlot >
            </div>
            <RadialChart
                showLabels={true}
                data={goalieGraph}
                width={300}
                radius={100}
                height={300} />
            <div className='lineup-container'>
              <Lineup lines={team.lines}></Lineup>
            </div>
          </Tab.Pane>
      };
    });
    return (
      <Segment>
        <Tab
          grid={{ paneWidth: 13, tabWidth: 3 }}
          menu={{ fluid: true, vertical: true }}
          menuPosition='left'
          panes={panes}
        />
      </Segment>);
  }
}

const mapStateToProps = state => ({
  analysis: state.analysis.analysis,
});

const mapDispatchToProps = dispatch => ({
  getAnalysis: () => dispatch(actions.getAnalysis()),
});


export default connect(mapStateToProps, mapDispatchToProps)(Analysis);